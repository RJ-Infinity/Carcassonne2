using System;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using static System.Windows.Forms.AxHost;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PGL
{
    public class PGLForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer? components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.SkiaSurface = new PGLSKGLControl();
            this.SuspendLayout();
            // 
            // skglControl1
            // 
            SkiaSurface.BackColor = Color.Black;
            SkiaSurface.Dock = DockStyle.Fill;
            SkiaSurface.Location = new Point(0, 0);
            SkiaSurface.Margin = new Padding(4, 3, 4, 3);
            SkiaSurface.Name = "skglControl1";
            SkiaSurface.Size = new Size(800, 450);
            SkiaSurface.TabIndex = 0;
            SkiaSurface.VSync = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(SkiaSurface);
            ResumeLayout(false);
        }
        public PGLSKGLControl SkiaSurface { get; set; }

        public List<Layer> Layers = new();

        private Thread RenderThread;
        private AutoResetEvent ThreadGate;
        private bool alive = true;
#pragma warning disable CS8618
        public PGLForm()
        {
            InitializeComponent();
            Console.WriteLine(SkiaSurface.ParentForm == this);
            
               
            //SkiaSurface.MouseDown += SkiaSurface_MouseDown;
        }

        public static void CopyItem<U, T>(U source, T target)
        {
            // Need a way to rename the backing-field name to the property Name ("<A>k__BackingField" => "A")
            Func<string, string> renameBackingField = key => new string(key.Skip(1).Take(key.IndexOf('>') - 1).ToArray());

            // Get public source properties (change BindingFlags if you need to copy private memebers as well)
            var sourceProperties = source.GetType().GetProperties().ToDictionary(item => item.Name);
            // Get "missing" property setter's backing field
            var targetFields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField).ToDictionary(item => renameBackingField(item.Name));

            // Copy properties where target name matches the source property name
            foreach (var sourceProperty in sourceProperties)
            {
                if (targetFields.ContainsKey(sourceProperty.Key) == false)
                    continue; // No match. skip

                var sourceValue = sourceProperty.Value.GetValue(source);
                targetFields[sourceProperty.Key].SetValue(target, sourceValue);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Subscribe to the SKGLControl events
            SkiaSurface.PaintSurface += SkglControl1_PaintSurface;

            SkiaSurface.Click += SkglControl1_Click;
            SkiaSurface.DoubleClick += SkglControl1_DoubleClick;
            SkiaSurface.MouseCaptureChanged += SkglControl1_MouseCaptureChanged;
            SkiaSurface.MouseClick += SkglControl1_MouseClick;
            SkiaSurface.MouseDoubleClick += SkglControl1_MouseDoubleClick;
            SkiaSurface.MouseDown += SkglControl1_MouseDown;
            SkiaSurface.MouseEnter += SkglControl1_MouseEnter;
            SkiaSurface.MouseHover += SkglControl1_MouseHover;
            SkiaSurface.MouseLeave += SkglControl1_MouseLeave;
            SkiaSurface.MouseMove += SkglControl1_MouseMove;
            SkiaSurface.MouseUp += SkglControl1_MouseUp;
            SkiaSurface.MouseWheel += SkglControl1_MouseWheel;

            // Create a background rendering thread
            RenderThread = new Thread(RenderLoopMethod);
            ThreadGate = new AutoResetEvent(false);

            // Start the rendering thread
            RenderThread.Start();
        }
        private void SkglControl1_Click(object? sender, EventArgs e)=>OnClick(e);
        private void SkglControl1_DoubleClick(object? sender, EventArgs e)=>OnDoubleClick(e);
        private void SkglControl1_MouseCaptureChanged(object? sender, EventArgs e)=>OnMouseCaptureChanged(e);
        private void SkglControl1_MouseClick(object? sender, MouseEventArgs e)=>OnMouseClick(e);
        private void SkglControl1_MouseDoubleClick(object? sender, MouseEventArgs e)=>OnMouseDoubleClick(e);
        private void SkglControl1_MouseDown(object? sender, MouseEventArgs e)=>OnMouseDown(e);
        private void SkglControl1_MouseEnter(object? sender, EventArgs e)=>OnMouseEnter(e);
        private void SkglControl1_MouseHover(object? sender, EventArgs e)=>OnMouseHover(e);
        private void SkglControl1_MouseLeave(object? sender, EventArgs e)=>OnMouseLeave(e);
        private void SkglControl1_MouseMove(object? sender, MouseEventArgs e)=>OnMouseMove(e);
        private void SkglControl1_MouseUp(object? sender, MouseEventArgs e)=>OnMouseUp(e);
        private void SkglControl1_MouseWheel(object? sender, MouseEventArgs e)=>OnMouseWheel(e);

        protected override void OnClosing(CancelEventArgs e)
        {
            // Let the rendering thread terminate
            alive = false;
            ThreadGate.Set();

            base.OnClosing(e);
        }
        private void SkglControl1_PaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
        {
            // Clear the Canvas
            e.Surface.Canvas.Clear(SKColors.Black);

            // Paint each pre-rendered layer onto the Canvas using this GUI thread
            foreach (Layer layer in Layers)
            {
                layer.Paint(e.Surface.Canvas);
            }


            //using (SKPaint paint = new SKPaint())
            //{
            //    paint.Color = SKColors.LimeGreen;

                //for (int i = 0; i < Layers.Count; i++)
                //{
                //    Layer layer = Layers[i];
                //    string text = $"{layer.Title} - Renders = {layer.RenderCount}, Paints = {layer.PaintCount}";
                //    SKPoint textLoc = new SKPoint(10, 10 + (i * 15));

                //    e.Surface.Canvas.DrawText(text, textLoc, paint);
                //}

            //    paint.Color = SKColors.Cyan;
            //}
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            SKPoint point = new SKPoint(e.X, e.Y);
            for (int i = Layers.Count-1; i >= 0; i--)
            {
                if (
                    // if the point is in the layer the mouse down event is run
                    Layers[i].IsInLayer(point) &&
                    // if the mouseDown event returns true we stop the recursion
                    Layers[i].OnMouseDown(new EventArgs_Click(point,e.Button))
                )
                {
                    break;
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            SKPoint point = new SKPoint(e.X, e.Y);
            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                if (
                    Layers[i].IsInLayer(point) &&
                    Layers[i].OnMouseUp(new EventArgs_Click(point))
                )
                {
                    break;
                }
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            e.Delta
            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                if (
                    Layers[i].IsInLayer(point) &&
                    Layers[i].OnMouseUp(new EventArgs_Click(point))
                )
                {
                    break;
                }
            }
            base.OnMouseWheel(e);
        }
        protected override void OnResize(EventArgs e)
        {
            // Invalidate all of the Layers
            foreach (Layer layer in Layers)
            {
                layer.Invalidate();
            }

            // Start a new rendering cycle to redraw all of the layers.
            UpdateDrawing();
        }
        public virtual void UpdateDrawing()
        {
            // Unblock the rendering thread to begin a render cycle.  Only the invalidated
            // Layers will be re-rendered, but all will be repainted onto the SKGLControl.
            ThreadGate.Set();
        }
        private void RenderLoopMethod()
        {
            while (alive)
            {
                // Draw any invalidated layers using this Render thread
                DrawLayers();

                // Invalidate the SKGLControl to run the PaintSurface event on the GUI thread
                // The PaintSurface event will Paint the layer stack to the SKGLControl
                SkiaSurface.Invalidate();

                // DoEvents to ensure that the GUI has time to process
                Application.DoEvents();

                // Block and wait for the next rendering cycle
                ThreadGate.WaitOne();
            }
        }
        private void DrawLayers()
        {
            // Iterate through the collection of layers and raise the Draw event for each layer that is
            // invalidated.  Each event handler will receive a Canvas to draw on along with the Bounds for 
            // the Canvas, and can then draw the contents of that layer. The Draw commands are recorded and  
            // stored in an SKPicture for later playback to the SKGLControl.  This method can be called from
            // any thread.

            SKRect clippingBounds = SkiaSurface.ClientRectangle.ToSKRect();

            foreach (Layer layer in Layers)
            {
                layer.Render(clippingBounds);
            }
        }
    }
}
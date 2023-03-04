﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGL
{
    public class PanAndZoomLayer : Layer
    {
        public PanAndZoomLayer()
        {
            MaxZoom = 15;
            MinZoom = 0.15f;
        }
        protected List<PanAndZoomLayer> linked = new();
        public void AddLinkedLayer(PanAndZoomLayer layer)
        {
            layer.offset = Offset;
            layer.zoom = Zoom;
            layer.MaxZoom = MaxZoom;
            layer.MinZoom = MinZoom;
            layer.linked.Add(this);
            linked.Add(layer);
            layer.Invalidate();
        }
        public void RemoveLinkedLayer(PanAndZoomLayer layer)
        {
            layer.linked.Remove(this);
            linked.Remove(layer);
        }
        protected float zoom = 1;
        public float Zoom { get => zoom; }
        protected SKPoint offset = new(0,0);
        public SKPoint Offset
        {
            get => offset;
        }
        protected float maxZoom;
        public float MaxZoom
        {
            get => maxZoom;
            set
            {
                maxZoom = value;
                linked.ForEach((PanAndZoomLayer pzl) => pzl.maxZoom = value);
            }
        }
        protected float minZoom;
        public float MinZoom
        {
            get => minZoom;
            set
            {
                minZoom = value;
                linked.ForEach((PanAndZoomLayer pzl) => pzl.minZoom = value);
            }
        }
        public void Pan(SKPoint p)
        {
            offset += p;
            linked
            .Where((PanAndZoomLayer pzl) => pzl.Offset != Offset)
            .ToList()
            //.ForEach((PanAndZoomLayer pzl)=>Console.WriteLine(pzl.Offset));
            .ForEach((PanAndZoomLayer pzl) => {
                pzl.offset = Offset;
                pzl.Invalidate();
            });
            Invalidate();
        }
        public void ZoomAt(float amount, SKPoint c)
        {
            SKPoint InitialWorldPos = ScreenToWorld(c);
            zoom += amount;
            if (zoom > MaxZoom)
            {
                zoom = MaxZoom;
            }
            if (zoom < MinZoom)
            {
                zoom = MinZoom;
            }
            linked
            .Where((PanAndZoomLayer pzl) => pzl.Zoom != Zoom)
            .ToList()
            .ForEach((PanAndZoomLayer pzl) => {
                pzl.zoom = Zoom;
                pzl.Invalidate();
            });
            Pan(c - WorldToScreen(InitialWorldPos));
        }
        public SKPoint WorldToScreen(SKPoint w)
        {
            SKPoint rv = w;
            rv.X *= zoom;
            rv.Y *= zoom;
            rv += offset;
            return rv;
        }
        public SKRect WorldToScreen(SKRect w)
        {
            SKPoint TL = WorldToScreen(w.Location);
            SKPoint BR = WorldToScreen(w.Location+w.Size);
            return new SKRect(TL.X, TL.Y, BR.X, BR.Y);
        }
        public SKPoint ScreenToWorld(SKPoint s)
        {
            SKPoint rv = s - offset;
            rv.X /= zoom;
            rv.Y /= zoom;
            return rv;
        }
        public SKRect ScreenToWorld(SKRect w)
        {
            SKPoint TL = ScreenToWorld(w.Location);
            SKPoint BR = ScreenToWorld(w.Location + w.Size);
            return new SKRect(TL.X, TL.Y, BR.X, BR.Y);
        }
        public virtual bool AllowZoom(EventArgs_Scroll e) => true;
        public override bool OnMouseWheel(EventArgs_Scroll e)
        {
            if (AllowZoom(e)) {
                ZoomAt(e.Clicks * 0.1f, e.Position);
            }
            return base.OnMouseWheel(e);
        }
        public virtual bool AllowPanStart(EventArgs_Click e) => true;
        private bool Panning = false;
        public override bool OnMouseDown(EventArgs_Click e)
        {
            if (AllowPanStart(e)) {
                Panning = true;
                lastPos = e.Position;
            }
            return base.OnMouseDown(e);
        }
        SKPoint lastPos;
        public override bool OnMouseMove(EventArgs_MouseMove e)
        {
            if (Panning) {
                Pan(e.Position - lastPos);
                lastPos = e.Position;
            }
            return base.OnMouseMove(e);
        }
        public virtual bool AllowPanEnd(EventArgs_Click e) { return true; }
        public override bool OnMouseUp(EventArgs_Click e)
        {
            if (AllowPanEnd(e)) { Panning = false; }
            return base.OnMouseUp(e);
        }
    }
}

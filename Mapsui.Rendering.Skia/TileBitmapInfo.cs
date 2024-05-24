﻿using Mapsui.Extensions;
using SkiaSharp;
using System;

namespace Mapsui.Rendering.Skia;

public enum SKiaTileType
{
    Bitmap,
    Picture
}

public sealed class TileBitmapInfo : IDisposable
{
    private SKImage? _image;
    private SKPicture? _picture;

    public SKiaTileType Type { get; private set; }

    public SKImage? Bitmap
    {
        get
        {
            if (Type == SKiaTileType.Bitmap)
                return _image;
            else
                return null;
        }
        set
        {
            _image = value;
            Type = SKiaTileType.Bitmap;
        }
    }

    public SKPicture? Picture
    {
        get
        {
            if (Type == SKiaTileType.Picture)
                return _picture;
            else
                return null;
        }
        set
        {
            _picture = value;
            Type = SKiaTileType.Picture;
        }
    }

    public long IterationUsed { get; set; }
    public float Width => Bitmap?.Width ?? Picture?.CullRect.Width ?? 0;
    public float Height => Bitmap?.Height ?? Picture?.CullRect.Height ?? 0;

    public bool IsDisposed => _image == null;

    public void Dispose()
    {
        DisposableExtension.DisposeAndNullify(ref _image);
    }
}


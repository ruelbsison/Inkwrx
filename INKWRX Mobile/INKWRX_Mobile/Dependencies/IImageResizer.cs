﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INKWRX_Mobile.Dependencies
{
    public interface IImageResizer
    {
        Task<byte[]> ResizeImage(byte[] image, double factor);
    }
}

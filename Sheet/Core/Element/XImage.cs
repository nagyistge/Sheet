﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Sheet.Core
{
    public class XImage : XElement
    {
        public XImage(object element)
        {
            Native = element;
        }
    }
}

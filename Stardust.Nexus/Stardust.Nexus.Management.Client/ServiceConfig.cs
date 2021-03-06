﻿//
// ServicesConfig.cs
// This file is part of Stardust
//
// Author: Jonas Syrstad (jsyrstad2+StardustCore@gmail.com), http://no.linkedin.com/in/jonassyrstad/) 
// Copyright (c) 2014 Jonas Syrstad. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Stardust.Interstellar.Config
{
    public class ServiceConfig
    {
        public long Id { get; set; }

        public string ServiceName { get; set; }

        public List<ConfigParameter> ConnectionStrings { get; set; }

        private readonly ConcurrentDictionary<string, ConfigParameter> ParameterCache = new ConcurrentDictionary<string, ConfigParameter>(); 

        public List<ConfigParameter> Parameters { get; set; }

        public string GetConfigParameter(string name)
        {
            var val = GetValue(name);
            if (val==null) return "";
            return val.Value;
        }

        private ConfigParameter GetValue(string name)
        {
            ConfigParameter param;
            if (ParameterCache.TryGetValue(string.Format("{0}_{1}", ServiceName, name).ToLower(), out param)) return param;
            param = (from p in Parameters where Equals(p.Name, name) select p).FirstOrDefault();
            ParameterCache.TryAdd(string.Format("{0}_{1}", ServiceName, name).ToLower(), param);
            return param;
        }

        public IdentitySettings IdentitySettings { get; set; }
    }
}

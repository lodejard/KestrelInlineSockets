// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionItemsFeature
    {
        private IDictionary<object, object> _items = new Dictionary<object, object>();

        IDictionary<object, object> IConnectionItemsFeature.Items
        {
            get => _items;
            set => _items = value;
        }
    }
}

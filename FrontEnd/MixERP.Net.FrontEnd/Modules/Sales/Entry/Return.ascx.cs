﻿/********************************************************************************
Copyright (C) Binod Nepal, Mix Open Foundation (http://mixof.org).

This file is part of MixERP.

MixERP is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

MixERP is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with MixERP.  If not, see <http://www.gnu.org/licenses/>.
***********************************************************************************/

using MixERP.Net.Common;
using MixERP.Net.Common.Models.Transactions;
using MixERP.Net.Core.Modules.Sales.Resources;
using MixERP.Net.FrontEnd.Base;
using MixERP.Net.FrontEnd.UserControls.Products;
using System;

namespace MixERP.Net.Core.Modules.Sales.Entry
{
    public partial class Return : MixERPUserControl
    {
        public override void OnControlLoad(object sender, EventArgs e)
        {
            long tranId = Conversion.TryCastLong(this.Request.QueryString["TranId"]);
            if (tranId <= 0)
            {
                this.Response.Redirect("~/Modules/Sales/Return.mix");
            }

            using (ProductControl product = (ProductControl)this.Page.LoadControl("~/UserControls/Products/ProductControl.ascx"))
            {
                product.Book = TranBook.Sales;
                product.SubBook = SubTranBook.Return;
                product.Text = Titles.SalesReturn;
                product.ShowPriceTypes = true;
                product.ShowStore = true;
                product.ShowSalesType = true;
                product.Initialize();

                this.Placeholder1.Controls.Add(product);
            }

            base.OnControlLoad(sender, e);
        }
    }
}
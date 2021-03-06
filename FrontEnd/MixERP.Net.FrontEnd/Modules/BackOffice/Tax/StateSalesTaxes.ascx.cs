﻿using MixERP.Net.Common.Helpers;
using MixERP.Net.Core.Modules.BackOffice.Resources;
using MixERP.Net.FrontEnd.Base;
using MixERP.Net.WebControls.ScrudFactory;

/********************************************************************************
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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MixERP.Net.Core.Modules.BackOffice.Tax
{
    public partial class StateSalesTaxes : MixERPUserControl
    {
        public override void OnControlLoad(object sender, EventArgs e)
        {
            using (ScrudForm scrud = new ScrudForm())
            {
                scrud.KeyColumn = "state_sales_tax_id";
                scrud.TableSchema = "core";
                scrud.Table = "state_sales_taxes";
                scrud.ViewSchema = "core";
                scrud.View = "state_sales_taxes";
                scrud.Text = Titles.StateSalesTaxes;

                scrud.DisplayFields = GetDisplayFields();
                scrud.DisplayViews = GetDisplayViews();

                scrud.ResourceAssembly = Assembly.GetAssembly(typeof(StateSalesTaxes));
                this.ScrudPlaceholder.Controls.Add(scrud);
            }
        }

        private static string GetDisplayFields()
        {
            List<string> displayFields = new List<string>();
            ScrudHelper.AddDisplayField(displayFields, "core.states.state_id", ConfigurationHelper.GetDbParameter("StateDisplayField"));
            ScrudHelper.AddDisplayField(displayFields, "core.entities.entity_id", ConfigurationHelper.GetDbParameter("EntityDisplayField"));
            ScrudHelper.AddDisplayField(displayFields, "core.industries.industry_id", ConfigurationHelper.GetDbParameter("IndustryDisplayField"));
            ScrudHelper.AddDisplayField(displayFields, "core.item_groups.item_group_id", ConfigurationHelper.GetDbParameter("ItemGroupDisplayField"));
            return string.Join(",", displayFields);
        }

        private static string GetDisplayViews()
        {
            List<string> displayViews = new List<string>();
            ScrudHelper.AddDisplayView(displayViews, "core.states.state_id", "core.states");
            ScrudHelper.AddDisplayView(displayViews, "core.entities.entity_id", "core.entities");
            ScrudHelper.AddDisplayView(displayViews, "core.industries.industry_id", "core.industries");
            ScrudHelper.AddDisplayView(displayViews, "core.item_groups.item_group_id", "core.item_groups");
            return string.Join(",", displayViews);
        }
    }
}
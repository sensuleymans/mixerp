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
using MixERP.Net.Common.Helpers;
using MixERP.Net.Common.Models.Office;
using MixERP.Net.DBFactory;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace MixERP.Net.Core.Modules.Finance.Data.Helpers
{
    public static class CashRepositories
    {
        public static bool CashRepositoryCodeExists(string cashRepositoryCode)
        {
            const string sql = "SELECT 1 FROM office.cash_repositories WHERE cash_repository_code=@CashRepositoryCode;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryCode", cashRepositoryCode);

                return DbOperation.GetDataTable(command).Rows.Count.Equals(1);
            }
        }

        public static decimal GetBalance(int cashRepositoryId, string currencyCode)
        {
            const string sql = "SELECT transactions.get_cash_repository_balance(@CashRepositoryId, @CurrencyCode);";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryId", cashRepositoryId);
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);
                return Conversion.TryCastDecimal(DbOperation.GetScalarValue(command));
            }
        }

        public static decimal GetBalance(int cashRepositoryId)
        {
            const string sql = "SELECT transactions.get_cash_repository_balance(@CashRepositoryId);";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryId", cashRepositoryId);
                return Conversion.TryCastDecimal(DbOperation.GetScalarValue(command));
            }
        }

        public static decimal GetBalance(string cashRepositoryCode)
        {
            const string sql =
                "SELECT transactions.get_cash_repository_balance(office.get_cash_repository_id_by_cash_repository_code(@CashRepositoryCode));";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryCode", cashRepositoryCode);
                return Conversion.TryCastDecimal(DbOperation.GetScalarValue(command));
            }
        }

        public static decimal GetBalance(string cashRepositoryCode, string currencyCode)
        {
            const string sql =
                "SELECT transactions.get_cash_repository_balance(office.get_cash_repository_id_by_cash_repository_code(@CashRepositoryCode), @CurrencyCode);";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryCode", cashRepositoryCode);
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);
                return Conversion.TryCastDecimal(DbOperation.GetScalarValue(command));
            }
        }

        public static Collection<CashRepository> GetCashRepositories()
        {
            const string sql = "SELECT * FROM office.cash_repositories;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                return GetCashRepositories(DbOperation.GetDataTable(command));
            }
        }

        public static Collection<CashRepository> GetCashRepositories(int officeId)
        {
            const string sql = "SELECT * FROM office.cash_repositories WHERE office_id=@OfficeId;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@OfficeId", officeId);
                return GetCashRepositories(DbOperation.GetDataTable(command));
            }
        }

        public static CashRepository GetCashRepository(int? cashRepositoryId)
        {
            CashRepository cashRepository = new CashRepository();

            if (cashRepositoryId != null && cashRepositoryId != 0)
            {
                const string sql = "SELECT * FROM office.cash_repositories WHERE cash_repository_id=@CashRepositoryId;";
                using (NpgsqlCommand command = new NpgsqlCommand(sql))
                {
                    command.Parameters.AddWithValue("@CashRepositoryId", cashRepositoryId);

                    using (DataTable table = DbOperation.GetDataTable(command))
                    {
                        if (table != null)
                        {
                            if (table.Rows.Count.Equals(1))
                            {
                                cashRepository = GetCashRepository(table.Rows[0]);
                            }
                        }
                    }
                }
            }

            return cashRepository;
        }

        public static DataTable GetCashRepositoryDataTable(int officeId)
        {
            return FormHelper.GetTable("office", "cash_repositories", "office_id", Conversion.TryCastString(officeId), "cash_repository_id");
        }

        private static Collection<CashRepository> GetCashRepositories(DataTable table)
        {
            Collection<CashRepository> cashRepositoryCollection = new Collection<CashRepository>();

            if (table == null || table.Rows.Count.Equals(0))
            {
                return cashRepositoryCollection;
            }

            foreach (DataRow row in table.Rows)
            {
                if (row != null)
                {
                    CashRepository cashRepository = GetCashRepository(row);

                    cashRepositoryCollection.Add(cashRepository);
                }
            }

            return cashRepositoryCollection;
        }

        private static CashRepository GetCashRepository(DataRow row)
        {
            CashRepository cashRepository = new CashRepository();

            cashRepository.CashRepositoryId =
                Conversion.TryCastInteger(DataRowHelper.GetColumnValue(row, "cash_repository_id"));
            cashRepository.OfficeId = Conversion.TryCastInteger(DataRowHelper.GetColumnValue(row, "office_id"));
            cashRepository.Office = GetOffice(cashRepository.OfficeId);
            cashRepository.CashRepositoryCode =
                Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "cash_repository_code"));
            cashRepository.CashRepositoryName =
                Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "cash_repository_name"));
            cashRepository.ParentCashRepositoryId =
                Conversion.TryCastInteger(DataRowHelper.GetColumnValue(row, "parent_cash_repository_id"));
            cashRepository.ParentCashRepository = GetCashRepository(cashRepository.ParentCashRepositoryId);
            cashRepository.Description = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "description"));

            return cashRepository;
        }

        private static Office GetOffice(int? officeId)
        {
            Office office = new Office();

            if (officeId != null && officeId != 0)
            {
                const string sql = "SELECT * FROM office.offices WHERE office_id=@OfficeId;";
                using (NpgsqlCommand command = new NpgsqlCommand(sql))
                {
                    command.Parameters.AddWithValue("@OfficeId", officeId);
                    using (DataTable table = DbOperation.GetDataTable(command))
                    {
                        if (table != null)
                        {
                            if (table.Rows.Count.Equals(1))
                            {
                                office = GetOffice(table.Rows[0]);
                            }
                        }
                    }
                }
            }

            return office;
        }

        private static Office GetOffice(DataRow row)
        {
            Office office = new Office();

            office.OfficeId = Conversion.TryCastInteger(DataRowHelper.GetColumnValue(row, "office_id"));
            office.OfficeCode = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "office_code"));
            office.OfficeName = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "office_name"));
            office.Nickname = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "nick_name"));
            office.RegistrationDate = Conversion.TryCastDate(DataRowHelper.GetColumnValue(row, "registration_date"));
            office.CurrencyCode = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "currency_code"));
            office.AddressLine1 = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "address_line_1"));
            office.AddressLine2 = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "address_line_2"));
            office.Street = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "street"));
            office.City = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "city"));
            office.State = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "state"));
            office.ZipCode = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "zip_code"));
            office.Country = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "country"));
            office.Phone = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "phone"));
            office.Fax = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "fax"));
            office.Email = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "email"));
            office.Url = new Uri(Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "url")),
                UriKind.RelativeOrAbsolute);
            office.RegistrationNumber =
                Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "registration_number"));
            office.PanNumber = Conversion.TryCastString(DataRowHelper.GetColumnValue(row, "pan_number"));
            office.ParentOfficeId = Conversion.TryCastInteger(DataRowHelper.GetColumnValue(row, "parent_office_id"));
            office.ParentOffice = GetOffice(office.ParentOfficeId);

            return office;
        }
    }
}
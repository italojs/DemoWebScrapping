using System;
using System.Data.SqlClient;
using DemoWebScrapping2.DAL;

namespace DemoWebScrapping2.Models
{
    public class Materialmodel
    {
        public string ProductName { get; set; }
        public string UrlPicture { get; set; }
        public string Description { get; set; }
        public string InitialBid { get; set; }
        public string BiggestBid { get; set; }
        public string QuantityBids { get; set; }

        public void Register()
        {
            var cmd = new SqlCommand();
            cmd.CommandText = "registerProducts";
            cmd.Parameters.AddWithValue("@UrlPicture", UrlPicture);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@InitialBid", InitialBid);
            cmd.Parameters.AddWithValue("@BiggestBid", BiggestBid);
            cmd.Parameters.AddWithValue("@QuantityBids", QuantityBids);
            cmd.Parameters.AddWithValue("@ProductName", ProductName);

            new SqlServer().IExecNonQueryProc(cmd);

        }
    }
}

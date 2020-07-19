using System;
using System.Collections.Generic;

namespace Rexobot.Gumroad
{
    public class GetSalesParams : QueryMap
    {
        public DateTime? After { get; set; }
        public DateTime? Before { get; set; }
        public string Email { get; set; }
        public int Page { get; set; } = 0;

        public override IDictionary<string, string> CreateQueryMap()
        {
            var dict = new Dictionary<string, string>();
            if (After != null) dict["after"] = ((DateTime)After).ToString("YYYY-MM-DD");
            if (After != null) dict["after"] = ((DateTime)Before).ToString("YYYY-MM-DD");
            if (!string.IsNullOrWhiteSpace(Email)) dict["email"] = Email;
            dict["page"] = Page.ToString();
            return dict;
        }
    }
}

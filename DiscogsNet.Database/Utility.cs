using System.Globalization;
using System.Text;

namespace DiscogsNet.Database
{
    class Utility
    {
        public static string RemoveDiacritics(string source)
        {
            string formD = source.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < formD.Length; i++)
            {
                UnicodeCategory uc = char.GetUnicodeCategory(formD[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(formD[i]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeevaDeleteApi
{ 
    public class VaultGrid
    {
        public class Table
        {
            public static readonly string Data = "data";
        }
        public class Column
        {
            /// <summary>
            /// symbolic name for id field
            /// </summary>
            public static readonly string IdFieldName = "id";

            /// <summary>
            /// symbolic name for major version number field
            /// </summary>
            public static readonly string MajorVersionFieldName = "major_version_number__v";

            /// <summary>
            /// symbolic name for minor version number field
            /// </summary>
            public static readonly string MinorVersionFieldName = "minor_version_number__v";

        }
    }
}

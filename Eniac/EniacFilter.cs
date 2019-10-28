using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacFilter
    {
        private String name;
        private int[] map;

        public EniacFilter(String n)
        {
            int i;
            name = n.ToUpper();
            map = new int[11];
            for (i = 0; i < 11; i++) map[i] = i;
        }

        public String getName()
        {
            return name;
        }

        public int[] getMap()
        {
            return map;
        }

        public void setMap(int[] m)
        {
            map = m;
        }

        public void save(StreamWriter file)
        {
            int i;
            file.WriteLine("filter " + name + " {");
            for (i = 0; i < 11; i++)
                if (map[i] != i) file.WriteLine("  " + (i + 1).ToString() + " " + (map[i] + 1).ToString());
            file.WriteLine("  }");
        }

        public void load(StreamReader file)
        {
            int pos;
            String line = "";
            int from, to;
            line = file.ReadLine().Trim().ToUpper();
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                pos = line.IndexOf(' ');
                from = Convert.ToInt32(line.Substring(0, pos));
                line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                to = Convert.ToInt32(line);
                map[from-1] = to-1;
                line = file.ReadLine().Trim().ToUpper();
            }
        }

        public int translate(int inp)
        {
            int i;
            int ret;
            int mask;
            mask = 1<<10;
            ret = 0;
            for (i = 0; i < 11; i++)
            {
                if ((inp & mask) == mask)
                {
                    ret |= (1 << (10-map[i]));
                }
                mask >>= 1;
            }
            return ret;
        }
    }
}

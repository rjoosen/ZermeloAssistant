using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZermeloFunction
{
    public class DataSets
    {
        private Dictionary<string, string> _lessons;
        public Dictionary<string, string> Lessons { get { return _lessons; } }

        public DataSets()
        {
            _lessons = new Dictionary<string, string>();
            //_lessons.Add("ne", "Nederlands");
            //_lessons.Add("mu", "Muziek");
            //_lessons.Add("en", "Engels");
            //_lessons.Add("lo", "Lichemalijke opvoeding");
            //_lessons.Add("du", "Duits");
            //_lessons.Add("gr", "Grieks");
            //_lessons.Add("gs", "Geschiedenis");
            //_lessons.Add("la", "Latijn");
            //_lessons.Add("ment", "Mentor uur");
            //_lessons.Add("ak", "Aardrijskdkunde");
            //_lessons.Add("na", "Natuurkunde");
            //_lessons.Add("wi", "Wiskunde");
            //_lessons.Add("bi", "Biologie");
            //_lessons.Add("ment", "Mentor uur");
            //_lessons.Add("fa", "Frans");
            //_lessons.Add("te", "Tekenen");
        }
    }
}

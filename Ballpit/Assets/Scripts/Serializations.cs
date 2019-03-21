

namespace InfoVis.MixedReality.Serializations
{
    [System.Serializable]
    public class TimeSeriesData
    {
        [System.Serializable]
        public class Dataset
        {
            public string label = null;
            public int[] data = { };
        }

        public string[] labels = { };
        public Dataset[] datasets = { };
    }
}

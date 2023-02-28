using System.ComponentModel.DataAnnotations;

namespace YOLOv4MLNet.DataStructures
{
    public class YoloV4Result
    {
        /// <summary>
        /// x1, y1, x2, y2 in page coordinates.
        /// <para>left, top, right, bottom.</para>
        /// </summary>
        /// 
        [Key]
        public int Id { get; set; }
        public float BBox0 { get; set; }
        public float BBox1 { get; set; }
        public float BBox2 { get; set; }
        public float BBox3 { get; set; }



        /// <summary>
        /// The Bbox category.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Confidence level.
        /// </summary>
        public float Confidence { get; set; }

    }
}

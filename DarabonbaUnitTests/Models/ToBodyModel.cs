using Darabonba;

namespace DaraUnitTests.Models
{
    public class ToBodyModel : Model
    {
        [NameInMap("ListAllMyBucketsResult")]
        public ListAllMyBucketsResult listAllMyBucketsResult { get; set; }

    }
}

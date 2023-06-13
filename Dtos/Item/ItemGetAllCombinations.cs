namespace WebApi.Dtos.Item
{
    public class ItemGetAllCombinations
    {
        public int[]? Variants {get;set;} = null;
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
        public string Name {get; set;} = string.Empty;

        public static implicit operator ItemGetAllCombinations?(List<string>? v)
        {
            throw new NotImplementedException();
        }
    }
}
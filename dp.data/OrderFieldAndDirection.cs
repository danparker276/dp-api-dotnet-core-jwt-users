namespace dp.data
{
    public class OrderFieldAndDirection
    {
        public OrderFieldAndDirection(string fieldName, OrderDirections? direction)
        {
            this.FieldName = fieldName;
            this.Direction = direction ?? OrderDirections.Ascending;
        }

        public string FieldName { get; set; }
        public OrderDirections Direction { get; set; }

        public enum OrderDirections
        {
            Ascending,
            Descending
        }
    }

    
}

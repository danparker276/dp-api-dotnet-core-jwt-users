namespace dp.api.Models
{
    public struct ClaimedUser
    {
        public int Id { get; set; }
        public bool IsAdmin { get; set; }

        //add as much here to the claims as you want
    }
}
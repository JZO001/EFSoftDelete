namespace EFSoftDelete.Models
{

    public class User : EntityBase
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public User(string userName, int? age)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (age.HasValue && age.Value < 0) throw new ArgumentOutOfRangeException(nameof(age));

            UserName = userName;
            if (age.HasValue) Age = age.Value;
        }

        public string UserName { get; init; }

        public int Age { get; set; }

    }

}

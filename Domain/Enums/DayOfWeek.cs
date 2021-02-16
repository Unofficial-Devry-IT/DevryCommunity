using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum DayOfWeek
    {
        [Display(Name = "Sunday")]
        SUNDAY = 0,
        [Display(Name = "Monday")]
        MONDAY = 1,
        [Display(Name = "Tuesday")]
        TUESDAY = 2,
        [Display(Name = "Wednesday")]
        WEDNESDAY = 3,
        [Display(Name = "Thursday")]
        THURSDAY = 4,
        [Display(Name = "Friday")]
        FRIDAY = 5,
        [Display(Name = "Saturday")]
        SATURDAY = 6
    }
}
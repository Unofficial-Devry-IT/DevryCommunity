using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum Month
    {
        [Display(Name = "January")]
        JANUARY = 1,
        [Display(Name = "February")]
        FEBRUARY = 2,
        [Display(Name = "March")]
        MARCH = 3,
        [Display(Name = "April")]
        APRIL = 4,
        [Display(Name = "May")]
        MAY = 5,
        [Display(Name = "June")]
        JUNE = 6,
        [Display(Name = "July")]
        JULY = 7,
        [Display(Name = "August")]
        AUGUST = 8,
        [Display(Name = "September")]
        SEPTEMBER = 9,
        [Display(Name = "October")]
        OCTOBER = 10,
        [Display(Name = "November")]
        NOVEMBER = 11,
        [Display(Name = "December")]
        DECEMBER = 12
    }
}
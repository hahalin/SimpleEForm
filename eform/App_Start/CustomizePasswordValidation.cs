using Microsoft.AspNet.Identity;

using System;

using System.Text.RegularExpressions;

using System.Threading.Tasks;
namespace eform.App_Start
{
    public class CustomizePasswordValidation : IIdentityValidator<string>
    {
        public int LengthRequired { get; set; }



        public CustomizePasswordValidation(int length)

        {

            LengthRequired = length;

        }



        public Task<IdentityResult> ValidateAsync(string Item)

        {

            if (String.IsNullOrEmpty(Item) || Item.Length < LengthRequired)

            {

                return Task.FromResult(IdentityResult.Failed(String.Format("密碼長度須至少為:{0}", LengthRequired)));

            }



            string PasswordPattern = @"^(?=.*[0-9])(?=.*[!@#$%^&*])[0-9a-zA-Z!@#$%^&*0-9]{10,}$";



            if (!Regex.IsMatch(Item, PasswordPattern))

            {

                return Task.FromResult(IdentityResult.Failed(String.Format("密碼需要一個非數字與字母的特殊字元")));

            }



            return Task.FromResult(IdentityResult.Success);

        }

    }
}
﻿using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity.Owin;

namespace Travelopedia.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request.Cookies["TimedCookie"] == null)
            {
                hiddenFieldLogin.Value = "login";
                RegisterHyperLink.NavigateUrl = "Register";
                // Enable this once you have account confirmation enabled for password reset functionality
                //ForgotPasswordHyperLink.NavigateUrl = "Forgot";

                OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
                var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
                if (!String.IsNullOrEmpty(returnUrl))
                {
                    RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
                }

                HttpCookie cookie = new HttpCookie("TimedCookie");
                cookie["User"] = User.Identity.Name;
                cookie.Expires = DateTime.Now.AddMinutes(5);

                Session["Timer"] = DateTime.Now;
                Session.Timeout = 10;
                Response.Cookies.Add(cookie);
            }
            else
            {
                Session.Timeout = 10;
                Session["Timer"] = DateTime.Now;
                Response.Redirect("~/Home.aspx");
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
//            Session.Abandon();
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                // This doen't count login failures towards account lockout
                // To enable password failures to trigger lockout, change to shouldLockout: true
                var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

                switch (result)
                {
                    case SignInStatus.Success:
                        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                        break;
                    case SignInStatus.LockedOut:
                        Response.Redirect("/Account/Lockout");
                        break;
                    case SignInStatus.RequiresVerification:
                        Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}", 
                                                        Request.QueryString["ReturnUrl"],
                                                        RememberMe.Checked),
                                          true);
                        break;
                    case SignInStatus.Failure:
                    default:
                        FailureText.Text = "Invalid login attempt";
                        ErrorMessage.Visible = true;
                        break;
                }
            }
        }
    }
}
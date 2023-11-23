using Microsoft.AspNetCore.Mvc;

namespace nvt_back
{
    public class Controller : ControllerBase
    {
        protected User _user
        {
            get
            {
                return (User)HttpContext.Items["loggedUser"];
            }
        }

    }

}

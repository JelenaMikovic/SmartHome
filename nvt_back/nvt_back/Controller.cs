using Microsoft.AspNetCore.Mvc;

namespace nvt_back
{
    public class Controller : ControllerBase
    {
        protected User _user => (User)HttpContext.Items["loggedUser"];

    }

}

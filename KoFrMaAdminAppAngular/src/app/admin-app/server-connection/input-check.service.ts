export class InputCheck
{
    username(username) : boolean
    {
        if(username == null || username == undefined || username == '')
        {
            alert("Invalid Username");
            return false
        }
        return true;
    }
    password(password) : boolean
    {
        if(password == undefined || password.length < 6)
        {
            alert("Password has to be at least 6 characters long");
            return false
        }
        return true;
    }
    email(email)
    {
        let regex = new RegExp(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/)
        if(!regex.test(email))
        {
            alert("Invalid Email");
            return false
        }
        return  true;
    }
}
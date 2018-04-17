export class InputCheck
{
    username(username : string) : boolean
    {
        username = username.trim();
        if(username == null || username == undefined || username == '')
        {
            alert("Invalid Username");
            return false;
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
    email(email) : boolean
    {
        let regex = new RegExp(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/)
        if(!regex.test(email))
        {
            alert("Invalid Email");
            return false
        }
        return  true;
    }
    isboolean(value : string) : boolean
    {
        if(value == 'true' || value == 'false')
        {
            return true;
        }
        alert("Only true or false")
        return false;
    }
}
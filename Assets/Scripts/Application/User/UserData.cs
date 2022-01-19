using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserData : MonoBehaviour
{

	#region Private variables

	private int passwordCode;
	private int UserID;
	private string userEmail;
	private string full_name;
	private int role_id;
	private string role_name;
	private string store_name;
	private string store_id;
	private string username;
	private string password;
	private string[] storeids;
	private string uniqueUserName;

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		initializeVariables();
		resetUserData();
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		//PlayerPrefs.DeleteAll();
		passwordCode = PlayerPrefs.GetInt("password");
	}

	#endregion

	#region Custom function - Set & Get complete list of user data from login credentials

	public void SetUserName(string name)
	{
		username = name;
	}

	public string GetUserName()
	{
		return username;
	}

	public void SetPassword(string pwd)
	{
		password = pwd;
	}

	public string GetPassword()
	{
		return password;
	}

	public void SetUserID(int userID)
	{
		UserID = userID;
	}

    public int GetUserID()
    {
        return UserID;
    }

	public void SetEmail(string email)
	{
		userEmail = email;
	}

    public string GetEmail()
    {
        return userEmail;
    }

	public void SetFullName(string fullName)
	{
		full_name = fullName;
	}

	public string GetFullName()
    {
        return full_name;
    }

	public void SetRoleID(int roleID)
	{
		role_id = roleID;
	}

	public int GetRoleID()
    {
        return role_id;
    }

	public void SetRoleName(string roleName)
	{
		role_name = roleName;
	}

	public string GetRoleName()
    {
        return role_name;
    }

	public void SetStoreID(string storeID)
	{
		store_id = storeID;
	}

	public string GetStoreID()
    {
        return store_id;
    }

	public void SetStoreName(string storeName)
	{
		store_name = storeName;
	}

	public string GetStoreName()
    {
        return store_name;
    }

	public void SetStoreIDs(string[] storeIDs)
	{
		storeids = storeIDs;
	}

    public string[] GetStoreIDs()
    {
        return storeids;
    }

	#endregion

	#region Custom function - Set & Get password change status code

	public void setPasswordCode(int code)
	{
		passwordCode = code;
		PlayerPrefs.SetInt("password", passwordCode);
	}

	public int getPasswordCode()
	{
		return passwordCode;
	}

	#endregion

	#region Custom function - Set & Get unique name of user

	public void setUniqueUserName(string name)
	{
		uniqueUserName = name;
	}

	public string getUniqueUserName()
	{
		return uniqueUserName;
	}

	#endregion

	#region Custom function - Reset user data from login credentials

	public void resetUserData()
	{
		SetUserID(0);
		SetEmail("");
		SetFullName("");
		SetRoleID(0);
		SetRoleName("");
		SetStoreID("");
		SetStoreName("");
	}

	#endregion

}

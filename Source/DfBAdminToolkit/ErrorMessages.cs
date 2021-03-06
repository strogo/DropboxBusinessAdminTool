﻿namespace DfBAdminToolkit
{
    public static class ErrorMessages
    {
        public readonly static string DLG_DEFAULT_TITLE = "Dropbox Business Admin Toolkit";
        public readonly static string MISSING_QUERYSTRING = "You must enter a query string first";
        public readonly static string INVALID_TOKEN = "You must provide a valid access token first";
        public readonly static string INVALID_TOKEN_AND_USER = "You must provide a valid access token, and select a user from the list";
        public readonly static string INVALID_DUMP_FILE_SELECTION = "You must choose at least one file to dump locally";
        public readonly static string INVALID_CSV_FILE = "Please provide a valid CSV file";
        public readonly static string MISSING_CSV_FILE = "Selected CSV file no longer exists";
        public readonly static string INVALID_CSV_DATA = "CSV file contains invalid data";
        public readonly static string FAILED_TO_ADD_MEMBER = "Bad request to the API, possibly due to no permissions on your app to add members.";
        public readonly static string FAILED_TO_REMOVE_MEMBER = "Bad request to the API, possibly due to no permissions on your app to remove members.";
        public readonly static string FAILED_TO_SUSPEND_MEMBER = "Bad request to the API, possibly due to no permissions on your app to suspend members.";
        public readonly static string FAILED_TO_UNSUSPEND_MEMBER = "Bad request to the API, possibly due to no permissions on your app to unsuspend members.";
        public readonly static string MISSING_ROLE = "Please select a user role first";
        public readonly static string MISSING_OUTPUT_FOLDER = "Please select output directory first";
        public readonly static string MISSING_FILES = "Please list file(s) first. You can do this by right-click and choose [ List file(s) ] option";
        public readonly static string CONFIRM_DELETE = "Are you sure you want to remove this list of members from your Dropbox Business account?";
        public readonly static string CONFIRM_SUSPEND = "Are you sure you want to suspend this list of members from your Dropbox Business account?";
        public readonly static string CONFIRM_UNSUSPEND = "Are you sure you want to unsuspend this list of members from your Dropbox Business account?";
        public readonly static string MISSING_TOKEN = "You must go to File->Settings to add your app tokens before performing any actions in the toolkit.";
        public readonly static string INVALID_EXPORT_FOLDER = "Directory to export report file is not found";
        public readonly static string FAILED_TO_GET_GROUPS = "Bad request to the API, possibly due to no permissions to get groups.";
        public readonly static string FAILED_TO_CREATE_GROUP = "Bad request to the API, possibly due to no permissions on your app to create groups.";
        public readonly static string FAILED_TO_ADD_MEMBER_TO_GROUP = "Bad request to the API, possibly due to no permissions on your app to add members to groups.";
        public readonly static string FAILED_TO_DELETE_MEMBER_FROM_GROUP = "Bad request to the API, possibly due to no permissions on your app to delete members from groups.";
    }
}
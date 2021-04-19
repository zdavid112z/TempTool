import connexion
import six

from swagger_server.models.file_variable import FileVariable  # noqa: E501
from swagger_server.models.file_variable_details import FileVariableDetails  # noqa: E501
from swagger_server import util


def files_file_name_delete(file_name):  # noqa: E501
    """Deletes a file

     # noqa: E501

    :param file_name: 
    :type file_name: str

    :rtype: None
    """
    return 'do some magic!'


def files_file_name_get(file_name):  # noqa: E501
    """Get a list of all the variables defined in a file

     # noqa: E501

    :param file_name: 
    :type file_name: str

    :rtype: List[FileVariable]
    """
    return 'do some magic!'


def files_file_name_post(body, file_name):  # noqa: E501
    """Uploads a file and fails if it already exists

     # noqa: E501

    :param body: The file contents in binary
    :type body: dict | bytes
    :param file_name: 
    :type file_name: str

    :rtype: None
    """
    if connexion.request.is_json:
        body = Object.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def files_file_name_put(body, file_name):  # noqa: E501
    """Uploads a file

     # noqa: E501

    :param body: The file contents in binary
    :type body: dict | bytes
    :param file_name: 
    :type file_name: str

    :rtype: None
    """
    if connexion.request.is_json:
        body = Object.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def files_file_name_variable_name_get(file_name, variable_name):  # noqa: E501
    """Returns the complete data for a variable

     # noqa: E501

    :param file_name: 
    :type file_name: str
    :param variable_name: 
    :type variable_name: str

    :rtype: FileVariableDetails
    """
    return 'do some magic!'


def files_get():  # noqa: E501
    """Returns a list of names of the files uploaded

     # noqa: E501


    :rtype: List[str]
    """
    return 'do some magic!'

# coding: utf-8

from __future__ import absolute_import

from flask import json
from six import BytesIO

from swagger_server.models.file_variable import FileVariable  # noqa: E501
from swagger_server.models.file_variable_details import FileVariableDetails  # noqa: E501
from swagger_server.test import BaseTestCase


class TestDefaultController(BaseTestCase):
    """DefaultController integration test stubs"""

    def test_files_file_name_delete(self):
        """Test case for files_file_name_delete

        Deletes a file
        """
        response = self.client.open(
            '/files/{fileName}'.format(file_name='file_name_example'),
            method='DELETE')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_files_file_name_get(self):
        """Test case for files_file_name_get

        Get a list of all the variables defined in a file
        """
        response = self.client.open(
            '/files/{fileName}'.format(file_name='file_name_example'),
            method='GET')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_files_file_name_post(self):
        """Test case for files_file_name_post

        Uploads a file and fails if it already exists
        """
        body = Object()
        response = self.client.open(
            '/files/{fileName}'.format(file_name='file_name_example'),
            method='POST',
            data=json.dumps(body),
            content_type='application/octet-stream')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_files_file_name_put(self):
        """Test case for files_file_name_put

        Uploads a file
        """
        body = Object()
        response = self.client.open(
            '/files/{fileName}'.format(file_name='file_name_example'),
            method='PUT',
            data=json.dumps(body),
            content_type='application/octet-stream')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_files_file_name_variable_name_get(self):
        """Test case for files_file_name_variable_name_get

        Returns the complete data for a variable
        """
        response = self.client.open(
            '/files/{fileName}/{variableName}'.format(file_name='file_name_example', variable_name='variable_name_example'),
            method='GET')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_files_get(self):
        """Test case for files_get

        Returns a list of names of the files uploaded
        """
        response = self.client.open(
            '/files',
            method='GET')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))


if __name__ == '__main__':
    import unittest
    unittest.main()

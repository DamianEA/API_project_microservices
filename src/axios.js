import axios from "axios";

export const axiosInstance = axios.create({
  //baseURL: 'https://some-domain.com/api/',
  baseURL: 'http://localhost:5  217/api/',
  timeout: 3000,
  headers: {'X-Custom-Header': 'foobar',
  'Content-Type': 'application/json'}
});


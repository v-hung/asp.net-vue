import axios from "axios";
import { accountApi } from "./api";

const axiosClient = axios.create({
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  console.log(token)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Create a single instance of AccountApi with axiosClient
// const accountApi = new AccountApi(undefined, undefined, axiosClient);

axiosClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response.status === 401) {
      accountApi
        .apiAccountRefreshPost({
          refreshToken: localStorage.getItem("refreshToken"),
        })
        .then((res) => {
          localStorage.setItem("token", res.data.token);
          error.config.headers.Authorization = `Bearer ${res.data.token}`;
          return axiosClient(error.config);
        })
        .catch(() => {
          return Promise.reject(error);
        });
    }
    return Promise.reject(error);
  }
);

export default axiosClient;

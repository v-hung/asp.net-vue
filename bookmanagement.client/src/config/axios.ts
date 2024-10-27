import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5046',
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response.status === 401) {
      const newToken = await refreshAccessToken();
      localStorage.setItem('token', newToken);
      error.config.headers.Authorization = `Bearer ${newToken}`;
      return apiClient(error.config);
    }
    return Promise.reject(error);
  }
);

const refreshAccessToken = async () => {
  const refreshToken = localStorage.getItem('refreshToken');
  const response = await apiClient.post('/api/Account/refresh', {
    refreshToken,
  });
  return response.data.token;
};

export default apiClient;

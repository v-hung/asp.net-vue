import axiosClient from '@/config/axios';
import { AccountApi, Configuration, MenuApi } from '@/generate-api';

export const BASE_PATH = "http://localhost:5046";

const config = new Configuration({
  basePath: BASE_PATH,
});

const accountApi = new AccountApi(config, undefined, axiosClient);
const menuApi = new MenuApi(config, undefined, axiosClient);

export { accountApi, menuApi };

import { onApiError } from '~/utils/api';
export default ({ store }) => onApiError(message => store.commit('SET_ERROR', message));

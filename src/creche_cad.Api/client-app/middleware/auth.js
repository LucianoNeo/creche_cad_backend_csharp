export default async ({ store, route, redirect }) => {
  await store.dispatch('auth/restore');
  if (!store.state.auth.isAuthenticated && route.path !== '/') return redirect('/');
  if (store.state.auth.isAuthenticated && route.path === '/') return redirect('/welcome');
};

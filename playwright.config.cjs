const { defineConfig } = require('@playwright/test');
module.exports = defineConfig({ testDir: './tests', workers: 1, timeout: 60000, reporter: [['list'], ['html', { open: 'never' }]],
  use: { baseURL: 'http://127.0.0.1:8082', viewport: { width: 1440, height: 1000 }, trace: 'retain-on-failure', screenshot: 'only-on-failure' } });

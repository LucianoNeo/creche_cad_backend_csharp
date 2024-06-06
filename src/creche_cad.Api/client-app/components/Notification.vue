<template>
    <v-snackbar v-model="notification.show" :color="notification.color" :timeout="notification.timeout">
      {{ notification.message }}
      <v-btn text @click="dismiss">Fechar</v-btn>
    </v-snackbar>
  </template>
  
  <script>
  export default {
    data() {
      return {
        notification: {
          show: false,
          message: '',
          color: 'success',
          timeout: 5000, // Tempo em milissegundos antes do snackbar desaparecer
        },
      };
    },
    methods: {
      showNotification(message, color = 'success', timeout = 5000) {
        this.notification.show = true;
        this.notification.message = message;
        this.notification.color = color;
        this.notification.timeout = timeout;
      },
      dismiss() {
        this.notification.show = false;
      },
    },
  };
  </script>
  
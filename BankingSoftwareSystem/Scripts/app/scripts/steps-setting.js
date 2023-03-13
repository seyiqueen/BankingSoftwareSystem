$(".tab-wizard").steps({
  headerTag: "h5",
  bodyTag: "section",
  transitionEffect: "fade",
  titleTemplate: '<span class="step">#index#</span> #title#',
  labels: {
    finish: "Submit"
  },
  onStepChanged: function (event, currentIndex, priorIndex) {
    $('.steps .current').prevAll().addClass('disabled');
  },
  onFinished: function (event, currentIndex) {
    $('#success-modal').modal('show');
  }
});

$(".registrationForm").steps({
  headerTag: "h5",
  bodyTag: "section",
  transitionEffect: "fade",
  titleTemplate: '<span class="step">#index#</span> <span class="info">#title#</span>',
  labels: {
    finish: "Register",
    next: "Next",
    previous: "Previous",
  },
    onStepChanging: function (e, currentIndex, newIndex) {
    var registrationPageAccountNumber = document.getElementById('registrationPageAccountNumber').value;
    var registrationPageAccountPIN = document.getElementById('registrationPageAccountPIN').value;
    var registrationPageAccountConfirmPIN = document.getElementById('registrationPageAccountConfirmPIN').value;
    var registrationPageAccountFirstName = document.getElementById('registrationPageAccountFirstName').value;
    var registrationPageAccountLastName = document.getElementById('registrationPageAccountLastName').value;

    if ((registrationPageAccountNumber.replace(/\s/g, "") == null || registrationPageAccountNumber.replace(/\s/g, "") == "") ||
      (registrationPageAccountPIN.replace(/\s/g, "") == null || registrationPageAccountPIN.replace(/\s/g, "") == "") ||
      (registrationPageAccountConfirmPIN.replace(/\s/g, "") == null || registrationPageAccountConfirmPIN.replace(/\s/g, "") == "") ||
      (registrationPageAccountFirstName.replace(/\s/g, "") == null || registrationPageAccountFirstName.replace(/\s/g, "") == "") ||
      (registrationPageAccountLastName.replace(/\s/g, "") == null || registrationPageAccountLastName.replace(/\s/g, "") == "")) {
      const formError = new CustomEvent('handleRegistrationFormValidation', {
        detail: {
          isSuccess: false,
          notificationMessage: 'All Fields are Required',
          showNotificationPanel: true,
          alertClass: 'alert-danger'
        }
      });
        document.dispatchEvent(formError);
        document.getElementById('errorNotificationPanel').style.display = 'block';
        document.getElementById('successNotificationPanel').style.display = 'none';
        document.getElementById('errorMessage').innerHTML = 'All Fields are Required';
      return false;
    }

    if (isNaN(registrationPageAccountPIN)) {
      const formError = new CustomEvent('handleRegistrationFormValidation', {
        detail: {
          isSuccess: false,
          notificationMessage: 'PIN must be Numbers only',
          showNotificationPanel: true,
          alertClass: 'alert-danger'
        }
      });
        document.dispatchEvent(formError);
        document.getElementById('errorNotificationPanel').style.display = 'block';
        document.getElementById('successNotificationPanel').style.display = 'none';
        document.getElementById('errorMessage').innerHTML = 'PIN must be Numbers only';
      return false;
    }

    if (registrationPageAccountPIN.length != 6) {
      const formError = new CustomEvent('handleRegistrationFormValidation', {
        detail: {
          isSuccess: false,
          notificationMessage: 'PIN must be exactly 6 Digits',
          showNotificationPanel: true,
          alertClass: 'alert-danger'
        }
      });
        document.dispatchEvent(formError);
        document.getElementById('errorNotificationPanel').style.display = 'block';
        document.getElementById('successNotificationPanel').style.display = 'none';
        document.getElementById('errorMessage').innerHTML = 'PIN must be exactly 6 Digits';
      return false;
    }

    if (registrationPageAccountPIN !== registrationPageAccountConfirmPIN) {
      const formError = new CustomEvent('handleRegistrationFormValidation', {
        detail: {
          isSuccess: false,
          notificationMessage: 'Passwords do not match',
          showNotificationPanel: true,
          alertClass: 'alert-danger'
        }
      });
        document.getElementById('errorNotificationPanel').style.display = 'block';
        document.getElementById('successNotificationPanel').style.display = 'none';
        document.getElementById('errorMessage').innerHTML = 'Passwords do not match';
      document.dispatchEvent(formError);
      return false;
    }

    const formSuccess = new CustomEvent('handleRegistrationFormValidation', {
      detail: {
        isSuccess: false,
        notificationMessage: '',
        alertClass: '',
        showNotificationPanel: false,
      }
    });
        document.dispatchEvent(formSuccess);
        document.getElementById('errorNotificationPanel').style.display = 'none';
        document.getElementById('successNotificationPanel').style.display = 'none';
        document.getElementById('errorMessage').innerHTML = '';
    return true;
  },
  onFinishing: function (e, currentIndex) {
    var registrationPageUsername = document.getElementById('registrationPageUsername').value;
    var registrationPagePassword = document.getElementById('registrationPagePassword').value;
    if ((registrationPageUsername.replace(/\s/g, "") == null || registrationPageUsername.replace(/\s/g, "") == "") ||
      (registrationPagePassword.replace(/\s/g, "") == null || registrationPagePassword.replace(/\s/g, "") == "")) {
      const pageTwoFormError = new CustomEvent('handleRegistrationFormValidation', {
        detail: {
          isSuccess: false,
          notificationMessage: 'All Fields are Required',
          showNotificationPanel: true,
          alertClass: 'alert-danger'
        }
      });
        document.dispatchEvent(pageTwoFormError);
        document.getElementById('errorNotificationPanel').style.display = 'block';
        document.getElementById('successNotificationPanel').style.display = 'none';
        document.getElementById('errorMessage').innerHTML = 'All Fields are Required';
      return false;
    }
    const formvalidation = new CustomEvent('handleRegistrationFormValidation', {
      detail: {
        isSuccess: false,
        notificationMessage: '',
        showNotificationPanel: false,
        alertClass: ''
      }
    });
    document.dispatchEvent(formvalidation);
    return true;
  },
  onStepChanged: function (event, currentIndex, priorIndex) {
    $('.steps .current').prevAll().addClass('disabled');
  },
  onFinished: function (event, currentIndex) {
    //$('#success-modal-btn').trigger('click');
    var registrationPageAccountNumber = document.getElementById('registrationPageAccountNumber').value;
    var registrationPageAccountPIN = document.getElementById('registrationPageAccountPIN').value;
    var registrationPageAccountFirstName = document.getElementById('registrationPageAccountFirstName').value;
    var registrationPageAccountLastName = document.getElementById('registrationPageAccountLastName').value;
    var registrationPageUsername = document.getElementById('registrationPageUsername').value;
    var registrationPagePassword = document.getElementById('registrationPagePassword').value;

    let model = {
      accountNumber: registrationPageAccountNumber,
      pin: registrationPageAccountPIN,
      firstName: registrationPageAccountFirstName,
      lastName: registrationPageAccountLastName,
      userName: registrationPageUsername,
      password: registrationPagePassword
    }
    const formCompleted = new CustomEvent('handleRegistrationProcess', {
      detail: {
        model: model
      }
    });
      document.dispatchEvent(formCompleted);

      document.getElementById('registerForm').submit();
  }
});

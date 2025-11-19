import Swal from 'sweetalert2'

export function useConfirmWarning() {
  const confirmDialog = async (
    title = 'Confirm Action',
    message = 'Are you sure?',
    confirmButtonText = 'Confirm',
    options = {}
  ) => {
    const result = await Swal.fire({
      title: title,
      html: message, 
      icon: 'warning',
      showCloseButton: true,
      showCancelButton: true,
      confirmButtonText: confirmButtonText,
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#d33',
      cancelButtonColor: '#6c757d',
    })

    return result.isConfirmed
  }

  return { confirmDialog }
}

export function useConfirm() {
  const confirmDialog = async (
    title = 'Confirm Action',
    message = 'Are you sure?',
    confirmButtonText = 'Confirm',
    options = {}
  ) => {
    const result = await Swal.fire({
      title: title,
      html: message,
      icon: 'question',
      showCloseButton: true,
      showCancelButton: true,
      confirmButtonText: confirmButtonText,
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#6c757d',
    })

    return result.isConfirmed
  }

  return { confirmDialog }
}

export function useInputDialog() {
  const inputDialog = async (title, html, confirmButtonText, preConfirmFn) => {
    const result = await Swal.fire({
      title: title,
      html: html,
      icon: 'info',
      showCloseButton: true,
      showCancelButton: true,
      confirmButtonText: confirmButtonText,
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: 'gray',
      focusConfirm: false,

      preConfirm: preConfirmFn,
    });
    if (result.isConfirmed && result.value) {
      return result.value;
    }
    return null;
  };

  return { inputDialog };
}

export const successDialog = (message = "Operation completed successfully!", title = "Success") => {
  return Swal.fire({
    title,
    text: message,
    icon: "success",
    timer: 2000,
    timerProgressBar: true,
    showConfirmButton: false
  })
}

export const errorDialog = (message = "Something went wrong", title = "Error") => {
  return Swal.fire({
    title,
    text: message,
    icon: "error",
    confirmButtonColor: '#6c757d',
    confirmButtonText: "OK"
  })
}

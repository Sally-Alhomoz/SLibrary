import Swal from 'sweetalert2'

export const useConfirm = () => {
  const confirm = async (options = {}) => {
    const defaultOptions = {
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Yes',
      cancelButtonText: 'Cancel',
      reverseButtons: true,
      customClass: { popup: 'text-left' },
      width: '400px'
    }

    const result = await Swal.fire({
      ...defaultOptions,
      ...options
    })

    return result.isConfirmed
  }

  return { confirm }
}

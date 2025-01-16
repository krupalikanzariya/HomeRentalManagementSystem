document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('bookingForm');
    const sections = document.querySelectorAll('.form-section');
    const steps = document.querySelectorAll('.step');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const submitBtn = document.getElementById('submitBtn');
    const successMessage = document.getElementById('bookingSuccess');

    let currentSection = 0;

    // Initialize date inputs
    const checkInInput = document.getElementById('checkIn');
    const checkOutInput = document.getElementById('checkOut');
    const today = new Date().toISOString().split('T')[0];
    checkInInput.min = today;

    // Navigation functions
    function showSection(n) {
        sections[currentSection].classList.remove('active');
        steps[currentSection].classList.remove('active');
        
        currentSection = n;
        
        sections[n].classList.add('active');
        steps[n].classList.add('active');

        // Update buttons
        prevBtn.style.display = n === 0 ? 'none' : 'inline-block';
        nextBtn.style.display = n === sections.length - 1 ? 'none' : 'inline-block';
        submitBtn.style.display = n === sections.length - 1 ? 'inline-block' : 'none';
    }

    // Event listeners for navigation
    prevBtn.addEventListener('click', () => {
        if (currentSection > 0) showSection(currentSection - 1);
    });

    nextBtn.addEventListener('click', () => {
        const inputs = sections[currentSection].querySelectorAll('input[required], select[required]');
        let isValid = true;

        inputs.forEach(input => {
            if (!input.checkValidity()) {
                isValid = false;
                input.classList.add('is-invalid');
            } else {
                input.classList.remove('is-invalid');
            }
        });

        if (isValid && currentSection < sections.length - 1) {
            showSection(currentSection + 1);
        }
    });

    // Date validation
    checkInInput.addEventListener('change', function() {
        const checkInDate = new Date(this.value);
        const minCheckOutDate = new Date(checkInDate);
        minCheckOutDate.setDate(checkInDate.getDate() + 1);
        
        checkOutInput.min = minCheckOutDate.toISOString().split('T')[0];
        if (checkOutInput.value && new Date(checkOutInput.value) <= checkInDate) {
            checkOutInput.value = '';
        }
    });

    // Form submission
    form.addEventListener('submit', function(event) {
        event.preventDefault();
        
        if (!form.checkValidity()) {
            event.stopPropagation();
        } else {
            // Simulate form submission with loading state
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processing...';
            
            setTimeout(() => {
                form.style.display = 'none';
                successMessage.style.display = 'block';
            }, 1500);
        }
        
        form.classList.add('was-validated');
    });

    // Phone number validation
    const phoneInput = document.getElementById('phone');
    phoneInput.addEventListener('input', function() {
        this.value = this.value.replace(/[^\d+\-() ]/g, '');
    });

    // Initialize first section
    showSection(0);
});
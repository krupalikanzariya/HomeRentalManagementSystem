// Wishlist functionality
// document.querySelectorAll('.wishlist-btn').forEach(btn => {
//     btn.addEventListener('click', function(e) {
//         e.preventDefault();
//         const icon = this.querySelector('i');
//         icon.classList.toggle('bi-heart');
//         icon.classList.toggle('bi-heart-fill');
//         icon.style.color = icon.classList.contains('bi-heart-fill') ? '#FF6F61' : '';
//     });
// });
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM fully loaded and parsed');

    // Select all wishlist buttons
    const buttons = document.querySelectorAll('.wishlist-btn');
    console.log('Wishlist buttons found:', buttons);

    buttons.forEach(btn => {
        btn.addEventListener('click', function(e) {
            console.log('Wishlist button clicked!'); // Debug line

            // Prevent the button's default behavior (e.g., if it's inside a link or form)
            e.preventDefault();

            // Stop the event from propagating to parent elements (e.g., carousel controls)
            e.stopPropagation(); 

            // Access and toggle the icon
            const icon = this.querySelector('i');
            if (!icon) {
                console.error('No <i> element found inside the button');
                return;
            }

            console.log('Icon element:', icon);

            // Toggle classes and color
            icon.classList.toggle('bi-heart');
            icon.classList.toggle('bi-heart-fill');
            const isFilled = icon.classList.contains('bi-heart-fill');
            icon.style.color = isFilled ? '#FF6F61' : '';

            console.log('Class toggled:', icon.classList, 'Color:', icon.style.color);
        });
    });

    // If you have carousel controls, make sure they are not triggered when clicking the wishlist button
    const carouselElements = document.querySelectorAll('.carousel');
    carouselElements.forEach(carousel => {
        // Prevent carousel controls from navigating when clicking the wishlist button
        carousel.addEventListener('click', function(e) {
            if (e.target.closest('.wishlist-btn')) {
                e.stopPropagation(); // Prevent carousel from navigating if the wishlist button was clicked
            }
        });
    });
});




// Image error handling
document.querySelectorAll('img').forEach(img => {
    img.addEventListener('error', function() {
        this.src = 'https://via.placeholder.com/800x600?text=Image+Not+Available';
    });
});

// Initialize property carousel with smooth transitions
document.querySelectorAll('.carousel').forEach(carousel => {
    new bootstrap.Carousel(carousel, {
        interval: 5000,
        touch: true
    });
});

// Lazy loading for images
document.querySelectorAll('img[data-src]').forEach(img => {
    img.setAttribute('loading', 'lazy');
});
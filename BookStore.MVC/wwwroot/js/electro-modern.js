// Modern E-ticaret JavaScript Fonksiyonları

$(document).ready(function() {
    // Update cart and favorites count on page load
    updateCartCount();
    updateFavoritesCount();

    // Smooth scrolling
    $('a[href^="#"]').on('click', function(event) {
        var target = $(this.getAttribute('href'));
        if( target.length ) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 1000);
        }
    });

    // Search functionality
    $('.search-input').on('keypress', function(e) {
        if (e.which === 13) {
            performSearch();
        }
    });

    $('.search-btn').on('click', function() {
        performSearch();
    });

    // Add to cart animation
    $('.btn-add-cart').on('click', function(e) {
        var $btn = $(this);
        var $form = $btn.closest('form');
        var originalText = $btn.html();
        
        // Add loading state
        $btn.html('<span class="spinner"></span> Ekleniyor...');
        $btn.prop('disabled', true);
        
        // Submit form via AJAX
        $.ajax({
            url: $form.attr('action'),
            type: 'POST',
            data: $form.serialize(),
            success: function(response) {
                $btn.html('✓ Sepete Eklendi');
                $btn.removeClass('btn-primary-custom').addClass('btn-success');
                
                // Update cart count
                updateCartCount();
                
                // Show toast notification
                showToast('Ürün sepete eklendi!', 'success');
                
                // Reset after 2 seconds
                setTimeout(function() {
                    $btn.html(originalText);
                    $btn.removeClass('btn-success').addClass('btn-primary-custom');
                    $btn.prop('disabled', false);
                }, 2000);
            },
            error: function() {
                $btn.html('❌ Hata');
                $btn.removeClass('btn-primary-custom').addClass('btn-danger');
                
                showToast('Ürün sepete eklenirken hata oluştu!', 'error');
                
                // Reset after 2 seconds
                setTimeout(function() {
                    $btn.html(originalText);
                    $btn.removeClass('btn-danger').addClass('btn-primary-custom');
                    $btn.prop('disabled', false);
                }, 2000);
            }
        });
        
        e.preventDefault();
        return false;
    });

    // Favorite toggle
    $('.btn-favorite').on('click', function(e) {
        e.preventDefault();
        
        var $btn = $(this);
        var $icon = $btn.find('i');
        
        if ($icon.hasClass('far')) {
            $icon.removeClass('far').addClass('fas');
            $btn.addClass('active');
            showToast('Favorilere eklendi!', 'success');
        } else {
            $icon.removeClass('fas').addClass('far');
            $btn.removeClass('active');
            showToast('Favorilerden çıkarıldı!', 'info');
        }
    });

    // Product image hover effect
    $('.product-image').hover(
        function() {
            $(this).find('.product-actions').fadeIn(300);
        },
        function() {
            $(this).find('.product-actions').fadeOut(300);
        }
    );

    // Mobile menu toggle
    $('.navbar-toggler').on('click', function() {
        $(this).toggleClass('active');
    });

    // Auto-hide alerts
    $('.alert').delay(5000).fadeOut(500);

    // Lazy loading for images
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver(function(entries, observer) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                    imageObserver.unobserve(img);
                }
            });
        });

        document.querySelectorAll('img[data-src]').forEach(function(img) {
            imageObserver.observe(img);
        });
    }

    // Price formatting
    $('.price').each(function() {
        var price = parseFloat($(this).text());
        if (!isNaN(price)) {
            $(this).text('₺' + price.toFixed(2));
        }
    });

    // Quantity controls
    $('.quantity-btn').on('click', function() {
        var $input = $(this).siblings('.quantity-input');
        var currentVal = parseInt($input.val()) || 1;
        var minVal = parseInt($input.attr('min')) || 1;
        var maxVal = parseInt($input.attr('max')) || 99;

        if ($(this).hasClass('quantity-plus') && currentVal < maxVal) {
            $input.val(currentVal + 1);
        } else if ($(this).hasClass('quantity-minus') && currentVal > minVal) {
            $input.val(currentVal - 1);
        }

        updateCartTotal();
    });

    // Initialize tooltips
    if (typeof bootstrap !== 'undefined') {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});

// Search function
function performSearch() {
    var searchTerm = $('.search-input').val().trim();
    if (searchTerm.length > 0) {
        // Redirect to search results page
        window.location.href = '/Books?search=' + encodeURIComponent(searchTerm);
    }
}

// Toast notification function
function showToast(message, type = 'info') {
    var toastHtml = `
        <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'primary'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    
    // Create toast container if it doesn't exist
    if ($('.toast-container').length === 0) {
        $('body').append('<div class="toast-container position-fixed bottom-0 end-0 p-3"></div>');
    }
    
    var $toast = $(toastHtml);
    $('.toast-container').append($toast);
    
    // Initialize and show toast
    if (typeof bootstrap !== 'undefined') {
        var toast = new bootstrap.Toast($toast[0]);
        toast.show();
        
        // Remove toast element after it's hidden
        $toast.on('hidden.bs.toast', function() {
            $(this).remove();
        });
    }
}

// Update cart total (placeholder function)
function updateCartTotal() {
    // This would be implemented based on your cart logic
    console.log('Cart total updated');
}

// Product quick view
function quickView(productId) {
    // Implementation for product quick view modal
    console.log('Quick view for product:', productId);
}

// Add to wishlist
function addToWishlist(productId) {
    // Implementation for adding to wishlist
    showToast('Ürün favorilere eklendi!', 'success');
}

// Compare products
function addToCompare(productId) {
    // Implementation for product comparison
    showToast('Ürün karşılaştırmaya eklendi!', 'info');
}

// Newsletter subscription
function subscribeNewsletter() {
    var email = $('#newsletter-email').val();
    if (email && validateEmail(email)) {
        showToast('Bülten aboneliğiniz başarıyla oluşturuldu!', 'success');
        $('#newsletter-email').val('');
    } else {
        showToast('Lütfen geçerli bir e-posta adresi girin!', 'error');
    }
}

// Email validation
function validateEmail(email) {
    var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Loading state management
function showLoading($element) {
    $element.addClass('loading');
    $element.prop('disabled', true);
}

function hideLoading($element) {
    $element.removeClass('loading');
    $element.prop('disabled', false);
}

// Currency formatting
function formatCurrency(amount) {
    return '₺' + parseFloat(amount).toFixed(2);
}

// Update cart count
function updateCartCount() {
    $.ajax({
        url: '/Cart/GetCartCount',
        type: 'GET',
        success: function(data) {
            var count = data.count || 0;
            $('#cart-count').text(count);
            
            if (count > 0) {
                $('#cart-count').show();
            } else {
                $('#cart-count').hide();
            }
        },
        error: function() {
            $('#cart-count').text('0').hide();
        }
    });
}

// Update favorites count
function updateFavoritesCount() {
    // This would need to make an AJAX call to get actual favorites count
    // For now, just hide if 0
    var favCount = 0; // This should be fetched from server
    $('#favorites-count').text(favCount);
    
    if (favCount > 0) {
        $('#favorites-count').show();
    } else {
        $('#favorites-count').hide();
    }
}

// Call this function after adding items to cart
function refreshCounts() {
    updateCartCount();
    updateFavoritesCount();
}

// Debounce function for search
function debounce(func, wait, immediate) {
    var timeout;
    return function() {
        var context = this, args = arguments;
        var later = function() {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
    };
}

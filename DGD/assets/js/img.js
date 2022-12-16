function loab(img) {
    const url = img.getAttribute('lazy-src')

    img.setAttribute('src', url)
}



function ready() {
    if ('IntersectionObserver' in window) {
        var lazyImas = document.querySelectorAll('[lazy-src]')

        let observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    loab(entry.target)
                }
            })
        });
        lazyImas.forEach(img => {
            observer.observe(img)
        })
    } else {

    }
}

document.addEventListener('DOMContentLoaded', ready)
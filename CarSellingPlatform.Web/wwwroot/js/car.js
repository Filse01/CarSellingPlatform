document.addEventListener("DOMContentLoaded", function () {
    // Get all filter elements
    const searchInput = document.getElementById("searchInput");
    const brandFilter = document.getElementById("brandFilter");
    const minPriceFilter = document.getElementById("minPriceFilter");
    const maxPriceFilter = document.getElementById("maxPriceFilter");
    const minHpFilter = document.getElementById("minHpFilter");
    const maxHpFilter = document.getElementById("maxHpFilter");
    const categoryFilter = document.getElementById("categoryFilter");         // New
    const transmissionFilter = document.getElementById("transmissionFilter"); // New

    function loadCars(page = 1) {
        const params = new URLSearchParams({
            page: page,
            search: searchInput.value,
            brandId: brandFilter.value,
            categoryId: categoryFilter.value,                 // New
            transmissionTypeId: transmissionFilter.value,     // New
            minPrice: minPriceFilter.value,
            maxPrice: maxPriceFilter.value,
            minHp: minHpFilter.value,
            maxHp: maxHpFilter.value
        });

        fetch(`${baseUrl}?${params.toString()}`, {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
            .then(res => res.text())
            .then(html => {
                document.getElementById("carResults").innerHTML = html;
                // Re-bind pagination to preserve filters
                document.querySelectorAll("#carResults .page-link").forEach(link => {
                    link.addEventListener("click", function (e) {
                        e.preventDefault();
                        const pageNumber = new URL(link.href).searchParams.get("page");
                        loadCars(pageNumber);
                    });
                });
            })
            .catch(err => console.error("Error loading cars:", err));
    }

    // Add event listeners for text/number inputs
    [searchInput, minPriceFilter, maxPriceFilter, minHpFilter, maxHpFilter].forEach(input => {
        input.addEventListener("input", () => loadCars(1));
    });
    // Add event listeners for dropdowns
    [brandFilter, categoryFilter, transmissionFilter].forEach(select => {
        select.addEventListener("change", () => loadCars(1));
    });
});
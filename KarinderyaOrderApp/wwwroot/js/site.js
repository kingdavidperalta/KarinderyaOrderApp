const form = document.querySelector('[data-order-form]');
const cartBody = form.querySelector('[data-cart-body]');
const totalEl = form.querySelector('[data-order-total]');
const submitBtn = form.querySelector('[data-submit]');
const rowTemplate = document.querySelector('[data-cart-row]');
const emptyTemplate = document.querySelector('[data-cart-empty]');
const money = n => n.toLocaleString('en-PH', { minimumFractionDigits: 2, maximumFractionDigits: 2 });



document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-confirm-modal]').forEach(modal => {
        modal.addEventListener('show.bs.modal', event => {
            const button = event.relatedTarget;
            if (!button) return;

            modal.querySelector('[data-confirm-name]').textContent = button.dataset.confirmName;
            modal.querySelector('form').action = button.dataset.confirmAction;
        });
    });
});


const foods = new Map();
form.querySelectorAll('[data-food-card]').forEach(card => {
    const id = card.dataset.foodId;
    foods.set(id, {
        card,
        name: card.dataset.name,
        price: parseFloat(card.dataset.price),
        stock: parseInt(card.dataset.stock),
        qtyInput: form.querySelector(`[data-qty-for="${id}"]`),
        addBtn: card.querySelector('[data-add]')
    });
});

const getQty = food => parseInt(food.qtyInput.value) || 0;

const setQty = (id, qty) => {
    const food = foods.get(id);
    food.qtyInput.value = Math.max(0, Math.min(qty, food.stock));  
    render();
};

const render = () => {
    cartBody.replaceChildren();
    let total = 0;
    let count = 0;

    foods.forEach((food, id) => {
        const qty = getQty(food);

        // Update the food card
        food.card.classList.toggle('border-primary', qty > 0);
        food.addBtn.disabled = qty >= food.stock;
        food.addBtn.textContent = qty >= food.stock ? 'Max in cart'
            : qty > 0 ? `Add more (${qty} in cart)` : 'Add to cart';

        if (qty === 0) return;

        // Add a row to the order summary
        const lineTotal = qty * food.price;
        total += lineTotal;
        count++;

        const row = rowTemplate.content.firstElementChild.cloneNode(true);
        row.dataset.foodId = id;
        row.querySelector('[data-row-name]').textContent = food.name;
        row.querySelector('[data-row-qty]').textContent = qty;
        row.querySelector('[data-row-total]').textContent = money(lineTotal);
        row.querySelector('[data-inc]').disabled = qty >= food.stock;
        cartBody.appendChild(row);
    });

    if (count === 0) cartBody.appendChild(emptyTemplate.content.cloneNode(true));

    totalEl.textContent = money(total);
    submitBtn.disabled = count === 0;
};


form.addEventListener('click', event => {
    const addBtn = event.target.closest('[data-add]');
    if (!addBtn) return;
    const id = addBtn.closest('[data-food-card]').dataset.foodId;
    setQty(id, getQty(foods.get(id)) + 1);
});

cartBody.addEventListener('click', event => {
    const btn = event.target.closest('[data-inc], [data-dec], [data-remove]');
    if (!btn) return;
    const id = btn.closest('tr').dataset.foodId;
    const qty = getQty(foods.get(id));

    if (btn.matches('[data-inc]')) setQty(id, qty + 1);
    else if (btn.matches('[data-dec]')) setQty(id, qty - 1);
    else setQty(id, 0);
});

render();   
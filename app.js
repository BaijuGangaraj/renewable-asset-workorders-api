/* global $ */
$(function () {
  const apiUrl = "/api/workorders";

  function toDateInput(value) {
    return value ? value.slice(0, 10) : "";
  }

  function escapeHtml(value) {
    return $("<div>").text(value || "").html();
  }

  function statusClass(status) {
    return `status-${(status || "").toLowerCase().replaceAll(" ", "-")}`;
  }

  function priorityClass(priority) {
    return `priority-${(priority || "").toLowerCase()}`;
  }

  function formData() {
    return {
      workOrderNumber: $("#work-order-number").val(),
      assetName: $("#asset-name").val(),
      assetType: $("#asset-type").val(),
      site: $("#site").val(),
      priority: $("#priority").val(),
      status: $("#status").val(),
      reportedDate: $("#reported-date").val(),
      dueDate: $("#due-date").val(),
      estimatedCost: Number($("#estimated-cost").val()),
      description: $("#description").val()
    };
  }

  function resetForm() {
    $("#work-order-form")[0].reset();
    $("#work-order-id").val("");
    $("#reported-date").val(new Date().toISOString().slice(0, 10));
    $("#due-date").val(new Date().toISOString().slice(0, 10));
    $("#form-title").text("Create work order");
    $("#save-button").text("Create work order");
    $("#cancel-edit").addClass("hidden");
  }

  function showFormMessage(message, isError) {
    $("#form-message").text(message).toggleClass("error", Boolean(isError));
  }

  function renderOrders(orders) {
    const rows = orders.map(order => `
      <tr>
        <td><strong>${escapeHtml(order.workOrderNumber)}</strong><span class="muted">${escapeHtml(order.description)}</span></td>
        <td>${escapeHtml(order.assetName)}<span class="muted">${escapeHtml(order.assetType)} · ${escapeHtml(order.site)}</span></td>
        <td><span class="${priorityClass(order.priority)}">${escapeHtml(order.priority)}</span></td>
        <td><span class="pill ${statusClass(order.status)}">${escapeHtml(order.status)}</span></td>
        <td>${toDateInput(order.dueDate)}</td>
        <td>$${Number(order.estimatedCost).toLocaleString(undefined, { minimumFractionDigits: 2 })}</td>
        <td class="actions"><button class="table-button edit" type="button" data-id="${order.id}">Edit</button><button class="table-button danger delete" type="button" data-id="${order.id}">Delete</button></td>
      </tr>`);
    $("#work-orders").html(rows.length ? rows.join("") : "<tr><td colspan=\"7\">No work orders match this filter.</td></tr>");
  }

  function loadOrders() {
    const status = $("#status-filter").val();
    $.getJSON(apiUrl, status ? { status } : {})
      .done(renderOrders)
      .fail(() => $("#list-message").text("Unable to load work orders. Is the API running?").addClass("error"));
  }

  function editOrder(id) {
    $.getJSON(`${apiUrl}/${id}`).done(order => {
      $("#work-order-id").val(order.id);
      $("#work-order-number").val(order.workOrderNumber);
      $("#asset-name").val(order.assetName);
      $("#asset-type").val(order.assetType);
      $("#site").val(order.site);
      $("#priority").val(order.priority);
      $("#status").val(order.status);
      $("#reported-date").val(toDateInput(order.reportedDate));
      $("#due-date").val(toDateInput(order.dueDate));
      $("#estimated-cost").val(order.estimatedCost);
      $("#description").val(order.description);
      $("#form-title").text(`Edit ${order.workOrderNumber}`);
      $("#save-button").text("Save changes");
      $("#cancel-edit").removeClass("hidden");
      showFormMessage("", false);
      window.scrollTo({ top: 0, behavior: "smooth" });
    });
  }

  $("#work-order-form").on("submit", function (event) {
    event.preventDefault();
    const id = $("#work-order-id").val();
    const method = id ? "PUT" : "POST";
    const url = id ? `${apiUrl}/${id}` : apiUrl;
    showFormMessage("Saving…", false);
    $.ajax({ url, method, contentType: "application/json", data: JSON.stringify(formData()) })
      .done(() => { showFormMessage(`Work order ${id ? "updated" : "created"}.`, false); resetForm(); loadOrders(); })
      .fail(xhr => {
        const errors = xhr.responseJSON?.errors;
        const message = errors ? Object.values(errors).flat().join(" ") : "Unable to save the work order.";
        showFormMessage(message, true);
      });
  });

  $("#status-filter").on("change", loadOrders);
  $("#cancel-edit").on("click", () => { resetForm(); showFormMessage("Edit cancelled.", false); });
  $("#work-orders").on("click", ".edit", function () { editOrder($(this).data("id")); });
  $("#work-orders").on("click", ".delete", function () {
    const id = $(this).data("id");
    if (!window.confirm("Delete this work order?")) return;
    $.ajax({ url: `${apiUrl}/${id}`, method: "DELETE" })
      .done(() => { $("#list-message").removeClass("error").text("Work order deleted."); loadOrders(); })
      .fail(() => $("#list-message").addClass("error").text("Unable to delete the work order."));
  });

  resetForm();
  loadOrders();
});

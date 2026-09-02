import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EquipmentRequest, EquipmentRequestService } from './equipment-request.service';
@Component({
  imports: [ReactiveFormsModule],
  template: `<section>
    <h1>Create equipment request</h1>
    <form [formGroup]="form" (ngSubmit)="save()">
      <label>Equipment<input formControlName="item" /></label>
      @if (form.controls.item.touched && form.controls.item.invalid) {
        <small>Required, up to 100 characters</small>
      }
      <label>Business justification<textarea formControlName="justification"></textarea></label>
      @if (form.controls.justification.touched && form.controls.justification.invalid) {
        <small>Required, up to 1,000 characters</small>
      }
      <button [disabled]="form.invalid || loading()">
        {{ loading() ? 'Creating…' : 'Create draft' }}
      </button>
    </form>
    @if (error()) {
      <p class="error">{{ error() }}</p>
    }
  </section>`,
})
export class CreateRequestPage {
  private fb = inject(FormBuilder);
  private api = inject(EquipmentRequestService);
  private router = inject(Router);
  loading = signal(false);
  error = signal('');
  form = this.fb.nonNullable.group({
    item: ['', [Validators.required, Validators.maxLength(100)]],
    justification: ['', [Validators.required, Validators.maxLength(1000)]],
  });
  save() {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.api.create(this.form.getRawValue()).subscribe({
      next: (r) => this.router.navigate(['/requests', r.id]),
      error: (e) => {
        this.error.set(e.message);
        this.loading.set(false);
      },
    });
  }
}
@Component({
  imports: [ReactiveFormsModule],
  template: `<section>
    <h1>Request workspace</h1>
    @if (loading()) {
      <p>Loading…</p>
    }
    @if (error()) {
      <p class="error">{{ error() }}</p>
    }
    @if (request(); as r) {
      <article>
        <span class="status">{{ r.status }}</span>
        <h2>{{ r.item }}</h2>
        <p>{{ r.justification }}</p>
        <p>Version {{ r.version }}</p>
        <form [formGroup]="form">
          <label>Decision or completion note<textarea formControlName="reason"></textarea></label>
        </form>
        <div class="actions">
          @for (action of actions(r); track action) {
            <button (click)="transition(action)" [disabled]="saving()">{{ action }}</button>
          }
        </div>
        <h3>Audit history</h3>
        <ol>
          @for (h of r.history; track h.occurredAt) {
            <li>
              {{ h.occurredAt }} — {{ h.actorId }}: {{ h.previousStatus }} → {{ h.newStatus }}
              {{ h.reason || '' }}
            </li>
          }
        </ol>
      </article>
    }
  </section>`,
})
export class RequestPage {
  private route = inject(ActivatedRoute);
  private api = inject(EquipmentRequestService);
  loading = signal(true);
  saving = signal(false);
  error = signal('');
  request = signal<EquipmentRequest | null>(null);
  form = inject(FormBuilder).nonNullable.group({ reason: ['', [Validators.maxLength(500)]] });
  constructor() {
    this.load();
  }
  load() {
    this.api.get(this.route.snapshot.paramMap.get('id')!).subscribe({
      next: (r) => {
        this.request.set(r);
        this.loading.set(false);
      },
      error: (e) => {
        this.error.set(e.message);
        this.loading.set(false);
      },
    });
  }
  actions(r: EquipmentRequest) {
    return r.status === 'Draft'
      ? ['submit', 'cancel']
      : r.status === 'Submitted'
        ? ['approve', 'reject', 'cancel']
        : r.status === 'Approved'
          ? ['complete']
          : [];
  }
  transition(action: string) {
    const r = this.request();
    if (!r) return;
    this.saving.set(true);
    this.error.set('');
    this.api.transition(r.id, action, r.version, this.form.controls.reason.value).subscribe({
      next: (x) => {
        this.request.set(x);
        this.saving.set(false);
      },
      error: (e) => {
        this.error.set(e.message);
        this.saving.set(false);
        this.load();
      },
    });
  }
}
@Component({
  imports: [ReactiveFormsModule],
  template: `<section>
    <h1>Open a request</h1>
    <form [formGroup]="form" (ngSubmit)="open()">
      <label>Request ID<input formControlName="id" /></label
      ><button [disabled]="form.invalid">Open</button>
    </form>
  </section>`,
})
export class OpenRequestPage {
  private router = inject(Router);
  form = inject(FormBuilder).nonNullable.group({ id: ['', Validators.required] });
  open() {
    this.router.navigate(['/requests', this.form.controls.id.value]);
  }
}
@Component({
  imports: [ReactiveFormsModule],
  template: `<section>
    <h1>Connect your account</h1>
    <p>Paste a development JWT. It is kept only for this browser session.</p>
    <form [formGroup]="form" (ngSubmit)="save()">
      <label>Bearer token<textarea formControlName="token"></textarea></label
      ><button>Continue</button>
    </form>
  </section>`,
})
export class SignInPage {
  private router = inject(Router);
  form = inject(FormBuilder).nonNullable.group({ token: ['', Validators.required] });
  save() {
    sessionStorage.setItem('employee-operations-token', this.form.controls.token.value);
    this.router.navigate(['/requests/new']);
  }
}

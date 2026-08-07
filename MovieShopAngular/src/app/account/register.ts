import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from './auth.service';

@Component({ selector: 'app-register', imports: [ReactiveFormsModule, RouterLink], templateUrl: './register.html' })
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  readonly submitting = signal(false);
  readonly error = signal('');
  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required], lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    dateOfBirth: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.submitting.set(true); this.error.set('');
    this.auth.register(this.form.getRawValue()).pipe(finalize(() => this.submitting.set(false))).subscribe({
      next: () => void this.router.navigate(['/login'], { queryParams: { registered: true } }),
      error: (response: HttpErrorResponse) => this.error.set(response.error?.message || 'Unable to create account.')
    });
  }
}

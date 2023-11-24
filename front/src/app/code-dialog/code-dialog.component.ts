import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-code-dialog',
  templateUrl: './code-dialog.component.html',
  styleUrls: ['./code-dialog.component.css']
})
export class CodeDialogComponent implements OnInit {

  codeForm = new FormGroup({
    email: new FormControl('', [Validators.required]),
    code: new FormControl('', [Validators.required]),
  })


  constructor(
    public dialogRef: MatDialogRef<CodeDialogComponent>,
    private snackBar: MatSnackBar,
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
  }

  close() {
    this.dialogRef.close();
  }

  activate(){
    const dto = {
      email: this.codeForm.get('email')?.value,
      code: this.codeForm.get('code')?.value
    }
    
    this.authService.activate(dto).subscribe(
      (success) => {
        console.log(success)
        if (success) {
          this.router.navigate(['/login']);
        } else {
          this.snackBar.open('Invalid code', 'Close', { duration: 3000 });
        }
      },
      (error) => {
        console.error('Login error:', error);
        this.snackBar.open('An error occurred while activating in', 'Close', { duration: 3000 });
      }
    );
  }

}

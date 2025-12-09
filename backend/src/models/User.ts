import mongoose from 'mongoose';

export interface IUser {
  _id?: string;
  email: string;
  password: string;
  name: string;
  role: 'admin' | 'analyst' | 'viewer';
  createdAt: Date;
}

const userSchema = new mongoose.Schema<IUser>(
  {
    email: {
      type: String,
      required: true,
      unique: true,
    },
    password: {
      type: String,
      required: true,
    },
    name: {
      type: String,
      required: true,
    },
    role: {
      type: String,
      enum: ['admin', 'analyst', 'viewer'],
      default: 'viewer',
    },
  },
  { timestamps: true }
);

export const User = mongoose.model<IUser>('User', userSchema);
